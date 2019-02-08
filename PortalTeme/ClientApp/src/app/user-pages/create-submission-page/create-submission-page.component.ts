import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DropzoneFileUploadComponent } from '../../components/dropzone-file-upload/dropzone-file-upload.component';
import { ModelServiceFactory } from '../../services/model.service';
import { CreateTaskSubmissionRequest, StudentAssignedTask } from '../../models/assignment.model';
import { combineLatest, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { FormGroup, FormControl, AbstractControl } from '@angular/forms';
import { nameof } from '../../type-guards/nameof.guard';

@Component({
  selector: 'app-create-submission-page',
  templateUrl: './create-submission-page.component.html',
  styleUrls: ['./create-submission-page.component.scss']
})
export class CreateSubmissionPageComponent implements OnInit, OnDestroy {

  @ViewChild('dropzone') dropzone: DropzoneFileUploadComponent;
  constructor(private route: ActivatedRoute, private router: Router, private modelSvcFactory: ModelServiceFactory) { }

  private courseSlug: string;
  private assignmentSlug: string;
  private routeSub: Subscription;
  _uploadUrl: string = '/api/Files/Upload';

  private studentTask: StudentAssignedTask;

  submissionForm: FormGroup;

  ngOnInit() {

    this.routeSub = combineLatest(
      this.route.parent.paramMap,
      this.route.paramMap
    ).subscribe(([course, assignment]) => {
      this.courseSlug = course.get('slug');
      this.assignmentSlug = assignment.get('assigSlug');

      this.getAssignment();
    });

    this.submissionForm = new FormGroup({});
    this.submissionForm.addControl(nameof<CreateTaskSubmissionRequest>('description'), new FormControl(''));

  }

  private getAssignment() {
    this.modelSvcFactory.assignments.getBySlug(this.courseSlug, this.assignmentSlug)
      .pipe(take(1))
      .subscribe(assignmentResult => {

        this.modelSvcFactory.studentAssignedTasks.getAssignedTask(assignmentResult.id)
          .pipe(take(1))
          .subscribe(studentTask => {
            this.studentTask = studentTask;
          });
      });
  }

  get description(): AbstractControl {
    return this.submissionForm.get('description');
  }

  submit() {
    this.dropzone.uploadFiles()
      .then(files => {
        let submission: CreateTaskSubmissionRequest = {
          studentTaskId: this.studentTask.id,
          uploadedFiles: files,
          description: this.description.value
        }

        this.modelSvcFactory.submissions.create(submission);
        this.router.navigate(['../'], { relativeTo: this.route });
      });
  }

  ngOnDestroy(): void {
    this.routeSub && this.routeSub.unsubscribe();
  }
}
