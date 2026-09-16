using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eSSLAttendanceDesktopClient.Service
{
    public class AttendanceLogService : IAttendanceLogService
    {
        private readonly IAttendanceLogRepository _repository;
        private readonly IEmployeeRepository _employeeRepo;
        public AttendanceLogService()
        {
            _repository = new AttendanceLogRepository();
            _employeeRepo = new EmployeeRepository();
        }

        public Domain.AttendanceLog Create(Domain.AttendanceLog mAttendanceLog)
        {
            return _repository.Create(mAttendanceLog);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public Domain.AttendanceLogLister GetAll(Domain.AttendanceLogLister mLister)
        {
            mLister = _repository.GetAll(mLister);
            if (mLister != null && mLister.List != null && mLister.List.Count > default(int))
            {
                mLister.List = mLister.List.Select((log, index) =>
                {
                    log.BgColor = index % 2 == 0 ? 2 : 1;
                    return log;
                }).ToList();
            }
            return mLister;
        }

        public void ProcessLog(Domain.Device mDevice, List<Domain.AttendanceLog> mAttendanceLog)
        {
            if (mAttendanceLog == null || mAttendanceLog.Count == 0)
                throw new BadRequest("Attendance log must have a value!");

            if (mDevice == null || mDevice.Id == 0)
                throw new BadRequest("Device object must have a value!");

            var mEmployees = _employeeRepo.GetAllBy(mDevice.Id);

            // Associate logs with employees based on EnrollNumber
            mAttendanceLog.ForEach(m =>
            {
                var mEmployee = mEmployees.FirstOrDefault(e => e.DeviceUniqueId == m.EnrollNumber);
                if (mEmployee != null)
                {
                    m.EmployeeId = mEmployee.Id;
                    m.WorkingHours = mEmployee.WorkingHours; // Attach working hours from employee data
                }
            });

            // Sort logs by EmployeeId and Timestamp
            var sortedRecords = mAttendanceLog
                .Where(x => x.EmployeeId > 0) // Only valid Employee IDs
                .OrderBy(r => r.EmployeeId)
                .ThenBy(r => r.Timestamp)
                .ToList();

            var mAttendancePairs = new List<Domain.AttendancePair>();
            DateTime? lastCheckIn = null;
            int lastEmployeeId = 0; // Track EmployeeId to prevent mismatches
            TimeSpan standardWorkHours = TimeSpan.Zero;

            foreach (var record in sortedRecords)
            {
                if (lastCheckIn == null) // No active Check-In
                {
                    lastCheckIn = record.Timestamp;
                    lastEmployeeId = record.EmployeeId; // Store EmployeeId for validation
                    standardWorkHours = TimeSpan.FromHours(record.WorkingHours); // Fetch working hours from log data
                }
                else
                {
                    if (record.EmployeeId == lastEmployeeId) // Validate pairing with same EmployeeId
                    {
                        // Pair Check-In and Check-Out
                        var workDuration = record.Timestamp - lastCheckIn.Value;

                        mAttendancePairs.Add(new Domain.AttendancePair
                        {
                            EmployeeId = record.EmployeeId,
                            CheckIn = lastCheckIn.Value,
                            CheckOut = record.Timestamp,
                            Status = "Complete",
                            Overtime = workDuration > standardWorkHours ? workDuration - standardWorkHours : TimeSpan.Zero
                        });

                        // Reset Check-In
                        lastCheckIn = null;
                        lastEmployeeId = 0;
                    }
                    else
                    {
                        // Handle unpaired Check-In
                        mAttendancePairs.Add(new Domain.AttendancePair
                        {
                            EmployeeId = lastEmployeeId,
                            CheckIn = lastCheckIn.Value,
                            CheckOut = null,
                            Status = "Incomplete (Missing Check-Out)",
                            Overtime = TimeSpan.Zero
                        });

                        // Treat this record as a new Check-In
                        lastCheckIn = record.Timestamp;
                        lastEmployeeId = record.EmployeeId;
                        standardWorkHours = TimeSpan.FromHours(record.WorkingHours);
                    }
                }
            }

            // Ensure unpaired Check-In at the end
            if (lastCheckIn.HasValue)
            {
                mAttendancePairs.Add(new Domain.AttendancePair
                {
                    EmployeeId = lastEmployeeId,
                    CheckIn = lastCheckIn.Value,
                    CheckOut = null,
                    Status = "Incomplete (Missing Check-Out)",
                    Overtime = TimeSpan.Zero
                });
            }

            // Save attendance pairs to the repository
            _repository.BulkInsert(mAttendancePairs);
        }

        public void Update(int id, AttendanceLog mAttendanceLog)
        {
            if (mAttendanceLog != null)
            {
                _repository.Update(id, mAttendanceLog);
            }
            else
                throw new BadRequest("The Object must have a a value");
        }
    }
}
