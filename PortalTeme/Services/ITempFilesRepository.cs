using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface ITempFilesRepository {

        void AddTempFile(string fileName, TimeSpan absoluteExpiration = default(TimeSpan));

        TempFileInfo Dequeue();
        List<TempFileInfo> GetExpired();
        void ClearUntil(DateTime untilDate);
    }

    public class InMemoryTempFilesRepository : ITempFilesRepository {

        private SortedList<DateTime, TempFileInfo> queuedTempFiles;

        private ReaderWriterLockSlim locker;

        public InMemoryTempFilesRepository() {
            queuedTempFiles = new SortedList<DateTime, TempFileInfo>();
            locker = new ReaderWriterLockSlim();
        }

        public void AddTempFile(string fileName, TimeSpan absoluteExpiration = default(TimeSpan)) {
            locker.EnterWriteLock();
            try {
                AddFile(fileName, absoluteExpiration);
            } finally {
                locker.ExitWriteLock();
            }
        }

        private void AddFile(string fileName, TimeSpan absoluteExpiration) {
            if (absoluteExpiration == default(TimeSpan))
                absoluteExpiration = TimeSpan.FromMinutes(10);

            var tempFileInfo = new TempFileInfo {
                FileName = fileName,
                Expiration = DateTime.UtcNow.Add(absoluteExpiration)
            };
            queuedTempFiles.Add(tempFileInfo.Expiration, tempFileInfo);
        }

        public TempFileInfo Dequeue() {
            locker.EnterWriteLock();
            try {
                return DequeueInternal();
            } finally {
                locker.ExitWriteLock();
            }
        }

        private TempFileInfo DequeueInternal() {
            if (!queuedTempFiles.Any())
                return null;

            var firstKey = queuedTempFiles.Keys.First();
            queuedTempFiles.Remove(firstKey, out var value);
            return value;
        }

        public List<TempFileInfo> GetExpired() {
            locker.EnterReadLock();
            try {
                return GetExpiredInternal();
            } finally {
                locker.ExitReadLock();
            }
        }

        private List<TempFileInfo> GetExpiredInternal() {
            if (!queuedTempFiles.Any())
                return Enumerable.Empty<TempFileInfo>().ToList();

            return queuedTempFiles
                .TakeWhile(kvp => kvp.Key < DateTime.UtcNow)
                .Select(kvp => kvp.Value)
                .ToList();
        }

        public void ClearUntil(DateTime untilDate) {
            locker.EnterWriteLock();
            try {
                ClearUntilInternal(untilDate);
            } finally {
                locker.ExitWriteLock();
            }
        }

        private void ClearUntilInternal(DateTime untilDate) {
            if (!queuedTempFiles.Any())
                return;

            var file = queuedTempFiles.First();
            while (file.Key <= untilDate) {
                queuedTempFiles.Remove(file.Key);

                if (!queuedTempFiles.Any())
                    break;

                file = queuedTempFiles.First();
            }
        }
    }

    public class TempFileInfo {
        public string FileName { get; set; }

        public DateTime Expiration { get; set; }
    }
}
