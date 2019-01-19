import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, AbstractControl, FormControl, Validators } from '@angular/forms';

import { Observable, Subscription, BehaviorSubject, of } from 'rxjs';

import * as ClassicEditor from '../../../../ckeditor/ckeditor';
import { EditorConfig } from '../../../../typings/index';

import { nameof } from '../../../type-guards/nameof.guard';

import AssignmentTypeText, { Assignment, AssignmentEdit, AssignmentType } from '../../../models/assignment.model';

@Component({
  selector: 'app-assignment-edit-form',
  templateUrl: './assignment-edit-form.component.html',
  styleUrls: ['./assignment-edit-form.component.scss']
})
export class AssignmentEditFormComponent implements OnInit, OnDestroy {

  AssignmentTypes = AssignmentType;
  CKEditor = ClassicEditor;
  config: EditorConfig;

  assignmentForm: FormGroup;

  @Input() courseId: string;
  @Input() assignment: Observable<Assignment>;
  @Input() submitClick: (assignment: AssignmentEdit) => void;

  hasId: BehaviorSubject<boolean>;
  assignSub: Subscription;

  showNumberOfDuplicates: BehaviorSubject<boolean>;
  invalid: BehaviorSubject<boolean>;

  constructor() { }

  get isEditMode(): Observable<boolean> {
    return this.hasId;
  }

  get name(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('name'));
  }

  get assignmentType(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('type'));
  }

  get numberOfDuplicates(): AbstractControl {
    return this.assignmentForm.get(nameof<Assignment>('numberOfDuplicates'));
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

    this.initializeForm();
    this.watchFormChanges();

    if (this.assignment)
      this.watchAssignmentInputChanges();

    this.config = {
      language: 'ro'
    }

  }

  private initializeForm() {
    this.assignmentForm = new FormGroup({});
    this.assignmentForm.addControl(nameof<Assignment>('name'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('type'), new FormControl('0'));
    this.assignmentForm.addControl(nameof<Assignment>('numberOfDuplicates'), new FormControl(1, Validators.min(2)));
    this.assignmentForm.addControl(nameof<Assignment>('description'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('startDate'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('endDate'), new FormControl());
  }

  private watchAssignmentInputChanges() {
    this.assignSub = this.assignment
      .subscribe(assign => {
        if (assign.id) {
          this.hasId.next(true);
          this.courseId = assign.course.id;
        }
        this.name.setValue(assign.name);
        this.assignmentType.setValue(assign.type);
        this.numberOfDuplicates.setValue(assign.numberOfDuplicates);
        this.description.setValue(assign.description);
        this.startDate.setValue(assign.startDate);
        this.endDate.setValue(assign.endDate);
        Object.keys(this.assignmentForm.controls).forEach(ctrl => this.assignmentForm.get(ctrl).markAsTouched());
      });
  }

  private watchFormChanges() {
    this.showNumberOfDuplicates = new BehaviorSubject(true);
    this.invalid = new BehaviorSubject(false);

    this.assignmentType.valueChanges.subscribe((val: AssignmentType) => {
      const shouldShow = val == AssignmentType.MultipleChoiceList;
      if (this.showNumberOfDuplicates.value == shouldShow)
        return;

      if (shouldShow && this.numberOfDuplicates.disabled)
        this.numberOfDuplicates.enable();
      else if (!shouldShow && this.numberOfDuplicates.enabled)
        this.numberOfDuplicates.disable();

      this.showNumberOfDuplicates.next(shouldShow);
    });

    this.assignmentForm.statusChanges.subscribe(_ => {
      if (this.invalid.value != this.assignmentForm.invalid)
        this.invalid.next(this.assignmentForm.invalid);
    });
  }

  getAssignmentTypeText(type: AssignmentType) {
    return AssignmentTypeText[type];
  }

  get currentAssignmentTypeDetails() {
    var asignText = AssignmentType[this.assignmentType.value];
    return AssignmentTypeText[`${asignText}Text`];
  }

  create() {
    if (this.assignmentForm.invalid) {
      this.invalid.next(this.assignmentForm.invalid);
      return;
    }

    const newAssignment: AssignmentEdit = {
      name: this.name.value,
      type: this.assignmentType.value,
      numberOfDuplicates: this.numberOfDuplicates.value,
      description: this.description.value,
      course: {
        id: this.courseId
      },
      startDate: this.startDate.value,
      endDate: this.endDate.value
    };

    this.submitClick(newAssignment);
  }

  ngOnDestroy(): void {
    if (this.assignSub)
      this.assignSub.unsubscribe();
  }
}
