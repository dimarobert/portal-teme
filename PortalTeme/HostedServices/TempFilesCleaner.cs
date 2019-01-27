using Microsoft.Extensions.Hosting;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortalTeme.HostedServices {
    public class TempFilesCleaner : IHostedService, IDisposable {
        private readonly ITempFilesRepository tempFilesRepository;
        private readonly IFileProvider fileProvider;
        private Timer _timer;

        public TempFilesCleaner(ITempFilesRepository tempFilesRepository, IFileProvider fileProvider) {
            this.tempFilesRepository = tempFilesRepository;
            this.fileProvider = fileProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state) {

            var expired = tempFilesRepository.GetExpired();
            if (!expired.Any())
                return;

            foreach (var file in expired) {
                fileProvider.DeleteTempFile(file.FileName);
            }

            var last = expired.Last();
            tempFilesRepository.ClearUntil(last.Expiration);

        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() {
            _timer?.Dispose();
        }
    }
}
