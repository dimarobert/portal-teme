import { Component, OnInit, Input, Inject } from '@angular/core';
import { Assignment } from '../../../models/assignment.model';
import { MAT_DATE_LOCALE } from '@angular/material';

@Component({
  selector: 'app-assignment-card',
  templateUrl: './assignment-card.component.html',
  styleUrls: ['./assignment-card.component.scss']
})
export class AssignmentCardComponent implements OnInit {

  @Input() assignment: Assignment;

  constructor(@Inject(MAT_DATE_LOCALE) private matDateLocale: string) { }

  isHovered: boolean = false;

  ngOnInit() {
  }

  get submissionsStarted(): boolean {
    return Date.now() > this.assignment.startDate.valueOf();
  }

  get isPassedDeadline(): boolean {
    return Date.now() > this.assignment.endDate.valueOf();
  }

  get isCloseToDeadline(): boolean {
    return this.addDays(Date.now(), 2) > this.assignment.endDate.valueOf();
  }

  addDays(dateTimestamp: number, days: number): number {
    return dateTimestamp + (days * 86400000);
  }

  getDateString(date: Date): string {
    return date.toLocaleDateString(this.matDateLocale);
  }

}
