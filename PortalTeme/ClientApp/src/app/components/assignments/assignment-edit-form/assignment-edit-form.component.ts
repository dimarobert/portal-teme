import { Component, OnInit, Input, OnDestroy } from '@angular/core';

import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

import { EditorConfig } from '../../../../typings/index';
import { FormGroup, AbstractControl, FormControl } from '@angular/forms';
import { Assignment, AssignmentEdit } from '../../../models/assignment.model';
import { nameof } from '../../../type-guards/nameof.guard';
import { ModelServiceFactory } from '../../../services/model.service';
import { Observable, Subscription, BehaviorSubject, of } from 'rxjs';

@Component({
  selector: 'app-assignment-edit-form',
  templateUrl: './assignment-edit-form.component.html',
  styleUrls: ['./assignment-edit-form.component.scss']
})
export class AssignmentEditFormComponent implements OnInit, OnDestroy {

  CKEditor = ClassicEditor;
  config: EditorConfig;

  assignmentForm: FormGroup;

  @Input() courseId: string;
  @Input() assignment: Observable<Assignment>;

  hasId: BehaviorSubject<boolean>;
  assignSub: Subscription;

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  get isEditMode(): Observable<boolean> {
    return this.hasId;
  }

  get name(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('name'));
  }

  get description(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('description'));
  }

  get startDate(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('startDate'));
  }

  get endDate(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('endDate'));
  }

  ngOnInit() {
    this.hasId = new BehaviorSubject(false);
    this.assignmentForm = new FormGroup({});

    this.assignmentForm.addControl(nameof<Assignment>('name'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('description'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('startDate'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('endDate'), new FormControl());

    if (this.assignment) {
      this.assignSub = this.assignment
        .subscribe(assign => {
          if (assign.id) {
            this.hasId.next(true);
            this.courseId = assign.course.id;
          }

          this.name.setValue(assign.name);
          this.description.setValue(assign.description);
          this.startDate.setValue(assign.startDate);
          this.endDate.setValue(assign.endDate);
        });
    }

    this.config = {
      language: 'ro'
    }

  }

  protected create() {
    debugger;
    const newAssignment: AssignmentEdit = {
      name: this.name.value,
      description: this.description.value,
      course: {
        id: this.courseId
      },
      startDate: this.startDate.value,
      endDate: this.endDate.value
    };

    this.modelSvcFactory.assignments.save(newAssignment);
  }

  ngOnDestroy(): void {
    if (this.assignSub)
      this.assignSub.unsubscribe();
  }
}
