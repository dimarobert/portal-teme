import { PipeTransform, Pipe } from '@angular/core';

@Pipe({ name: 'keys', pure: true })
export class KeysPipe implements PipeTransform {
    transform(value, args: string[]): { key: string, value: any }[] {
        let keys = [];
        for (let key in value) {
            keys.push({ key: key, value: value[key] });
        }
        return keys;
    }
}