import { PipeTransform, Pipe } from '@angular/core';

@Pipe({ name: 'enumKeys', pure: true })
export class EnumKeysPipe implements PipeTransform {
    transform(enumValue, args: string[]): { key: string, value: any }[] {
        let keys = [];
        for (let enumKey in enumValue) {
            const numberKey = parseInt(enumKey, 10);
            const isValueProperty = numberKey >= 0
            if (isValueProperty) {
                keys.push({ key: numberKey, value: enumValue[enumKey] });
            }
        }
        return keys;
    }
}