import { PipeTransform, Pipe } from '@angular/core';

@Pipe({ name: 'diskSize', pure: true })
export class DiskSizePipe implements PipeTransform {
    private units = ['B', 'KB', 'MB', 'GB', 'TB'];

    transform(size: number, args: string[]): string {
        let unit = 0;
        while (size >= 1024) {
            size /= 1024.0;
            unit++;
        }

        let sizeStr = unit > 1
            ? size.toFixed(2)
            : Math.floor(size).toString();

        return `${sizeStr} ${this.units[unit]}`;
    }
}