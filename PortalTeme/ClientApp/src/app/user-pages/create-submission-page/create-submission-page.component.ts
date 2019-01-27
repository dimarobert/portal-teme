import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DropzoneFileUploadComponent } from '../../components/dropzone-file-upload/dropzone-file-upload.component';
import { ModelServiceFactory } from '../../services/model.service';
import { CreateTaskSubmissionRequest, StudentAssignedTask } from '../../models/assignment.model';
import { combineLatest, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

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
  protected uploadUrl: string = '/api/FileUpload';

  private studentTask: StudentAssignedTask;

  ngOnInit() {

    this.routeSub = combineLatest(
      this.route.parent.paramMap,
      this.route.paramMap
    ).subscribe(([course, assignment]) => {
      this.courseSlug = course.get('slug');
      this.assignmentSlug = assignment.get('assigSlug');

      this.getAssignment();
    });

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


  submit() {
    this.dropzone.uploadFiles()
      .then(files => {
        let submission: CreateTaskSubmissionRequest = {
          studentTaskId: this.studentTask.id,
          uploadedFiles: files
        }

        this.modelSvcFactory.studentAssignedTasks.submitTask(submission);
      });
  }

  ngOnDestroy(): void {
    this.routeSub && this.routeSub.unsubscribe();
  }
}
