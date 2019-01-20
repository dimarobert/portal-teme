import { Component, OnInit, Input } from '@angular/core';
import { Assignment } from '../../../models/assignment.model';

@Component({
  selector: 'app-assignment-card',
  templateUrl: './assignment-card.component.html',
  styleUrls: ['./assignment-card.component.scss']
})
export class AssignmentCardComponent implements OnInit {

  @Input() assignment: Assignment;

  constructor() { }

  protected isHovered: boolean = false;

  ngOnInit() {
  }

  isPassedDeadline(assignment: Assignment): boolean {
    return Date.now() > assignment.endDate.valueOf();
  }

  isCloseToDeadline(assignment: Assignment): boolean {
    return this.addDays(Date.now(), 2) > assignment.endDate.valueOf();
  }

  addDays(dateTimestamp: number, days: number): number {
    return dateTimestamp + (days * 86400000);
  }

}
