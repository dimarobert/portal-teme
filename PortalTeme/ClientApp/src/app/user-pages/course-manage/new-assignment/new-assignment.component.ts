import { Component, OnInit } from '@angular/core';

import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

import { EditorConfig } from '../../../../typings/index';
import { FormGroup, AbstractControl, FormControl } from '@angular/forms';
import { Assignment, AssignmentEdit } from '../../../models/assignment.model';
import { nameof } from '../../../type-guards/nameof.guard';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ModelServiceFactory } from '../../../services/model.service';

@Component({
  selector: 'app-new-assignment',
  templateUrl: './new-assignment.component.html',
  styleUrls: ['./new-assignment.component.scss']
})
export class NewAssignmentComponent implements OnInit {

  CKEditor = ClassicEditor;

  config: EditorConfig;

  assignmentForm: FormGroup;

  routeSub: Subscription;

  constructor(private route: ActivatedRoute, private modelSvcFractory: ModelServiceFactory) { }

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

    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const courseSlug = params.get('slug');

        // this.modelSvcFactory.assignments.getBySlug(this.assignmentSlug)
        //   .pipe(take(1))
        //   .subscribe(assignmentResult => {
        //     this.assignment = assignmentResult;
        //   });
      });

    this.assignmentForm = new FormGroup({});

    this.assignmentForm.addControl(nameof<Assignment>('name'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('description'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('startDate'), new FormControl());
    this.assignmentForm.addControl(nameof<Assignment>('endDate'), new FormControl());

    this.config = {
      language: 'ro'
    }

  }

  protected create() {
    const newAssignment: AssignmentEdit = {
      name: this.name.value,
      description: this.description.value,
      course: {
        id: ''
      },
      startDate: this.startDate.value,
      endDate: this.endDate.value
    };

    this.modelSvcFractory.assignments.save(newAssignment);
  }

}
