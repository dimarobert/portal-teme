import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl, AbstractControl } from '@angular/forms';

import * as ClassicEditor from '../../../../ckeditor/ckeditor';
import { EditorConfig } from '../../../../typings/index';

import { nameof } from '../../../type-guards/nameof.guard';
import { AssignmentTask, AssignmentTaskEdit } from '../../../models/assignment.model';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-assignment-tasks-edit-form',
  templateUrl: './assignment-tasks-edit-form.component.html',
  styleUrls: ['./assignment-tasks-edit-form.component.scss']
})
export class AssignmentTasksEditFormComponent implements OnInit {

  CKEditor = ClassicEditor;
  config: EditorConfig;

  @Input() assignmentId: string;
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

  get description(): AbstractControl {
    return this.taskForm.get(nameof<AssignmentTask>('description'));
  }

  create() {
    if (this.taskForm.invalid) {
      this.invalid.next(this.taskForm.invalid);
      return;
    }

    const newTask: AssignmentTaskEdit = {
      assignmentId: this.assignmentId,
      name: this.name.value,
      description: this.description.value
    };
    if (this.task.id)
      newTask.id = this.task.id;

    this.saving.next(true);
    this.submitClick(newTask)
      .then(_ => {
        this.saved.next(true);
        this.saving.next(false);
      });
  }

  delete() {
    this.deleteTask(this.task);
  }

}
