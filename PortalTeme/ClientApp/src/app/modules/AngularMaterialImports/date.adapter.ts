import { NativeDateAdapter, MAT_DATE_LOCALE } from '@angular/material';
import { Injectable, Optional, Inject } from '@angular/core';
import { Platform } from '@angular/cdk/platform';


@Injectable({
    providedIn: 'root'
})
export class DateAdapterService {

    private firstDay: number = 0;
    public setFirstDayOfWeek(day: number) {
        this.firstDay = day;
    }

    getFirstDayOfWeek(): number {
        return this.firstDay;
    }
}

@Injectable()
export class CustomDateAdapter extends NativeDateAdapter {

    constructor(@Optional() @Inject(MAT_DATE_LOCALE) matDateLocale: string, platform: Platform, private dateSvc: DateAdapterService) {
        super(matDateLocale, platform);
    }

    getFirstDayOfWeek(): number {
        return this.dateSvc.getFirstDayOfWeek();
    }

}