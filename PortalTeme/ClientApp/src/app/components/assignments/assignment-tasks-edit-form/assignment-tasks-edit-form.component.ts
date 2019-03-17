import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl, AbstractControl } from '@angular/forms';

import * as ClassicEditor from '../../../../ckeditor/ckeditor';
import { EditorConfig } from '../../../../typings/index';

import { nameof } from '../../../type-guards/nameof.guard';
import { AssignmentTask, AssignmentTaskEdit, AssignmentTaskCreateRequest, AssignmentTaskUpdateRequest } from '../../../models/assignment.model';
import { BehaviorSubject } from 'rxjs';
import { User } from '../../../models/course.model';

@Component({
  selector: 'app-assignment-tasks-edit-form',
  templateUrl: './assignment-tasks-edit-form.component.html',
  styleUrls: ['./assignment-tasks-edit-form.component.scss']
})
export class AssignmentTasksEditFormComponent implements OnInit {

  CKEditor = ClassicEditor;
  config: EditorConfig;

  @Input() courseStudents: User[];
  @Input() assignmentId: string;
  @Input() isCustomAssigned: boolean;
  @Input() task: AssignmentTask;
  @Input() submitClick: (task: AssignmentTaskEdit) => Promise<void>;
  @Input() deleteTask: (task: AssignmentTaskEdit) => void;

  taskForm: FormGroup;

  invalid: BehaviorSubject<boolean>;
  saved: BehaviorSubject<boolean>;
  saving: BehaviorSubject<boolean>;

  constructor() { }

  ngOnInit() {
    this.saved = new BehaviorSubject(true);
    this.saving = new BehaviorSubject(false);

    this.taskForm = new FormGroup({});
    this.taskForm.addControl(nameof<AssignmentTask>('name'), new FormControl());
    if (this.isCustomAssigned) {
      const assignedTo = this.task.studentsAssigned.length
        ? this.task.studentsAssigned[0].id
        : null;
      this.taskForm.addControl(nameof<AssignmentTaskCreateRequest>('assignedTo'), new FormControl(assignedTo));
    }
    this.taskForm.addControl(nameof<AssignmentTask>('description'), new FormControl());

    this.taskForm.valueChanges.subscribe(val => {
      this.saved.next(false);
    });

    if (this.task) {
      this.name.setValue(this.task.name);
      this.description.setValue(this.task.description);
    }

    this.config = {
      language: 'ro'
    }
  }

  get isEditMode(): boolean {
    return !!this.task.id;
  }

  get name(): AbstractControl {
    return this.taskForm.get(nameof<AssignmentTask>('name'));
  }

  get assignedTo(): AbstractControl {
    return this.taskForm.get(nameof<AssignmentTaskCreateRequest>('assignedTo'));
  }

  get description(): AbstractControl {
    return this.taskForm.get(nameof<AssignmentTask>('description'));
  }

  submitForm() {
    if (this.taskForm.invalid) {
      this.invalid.next(this.taskForm.invalid);
      return;
    }

    const request = this.createRequest();

    if (this.isCustomAssigned) {
      request.assignedTo = this.assignedTo.value;
    }

    this.saving.next(true);
    this.submitClick(request)
      .then(_ => {
        this.saved.next(true);
        this.saving.next(false);
      });
  }

  private createRequest(): AssignmentTaskCreateRequest {
    let request = <AssignmentTaskCreateRequest>{};
    if (this.task.id) {
      request = <AssignmentTaskUpdateRequest>{
        id: this.task.id
      };
    }

    request.assignmentId = this.assignmentId;
    request.name = this.name.value;
    request.description = this.description.value;

    return request;
  }

  delete() {
    this.deleteTask(this.task);
  }

}
