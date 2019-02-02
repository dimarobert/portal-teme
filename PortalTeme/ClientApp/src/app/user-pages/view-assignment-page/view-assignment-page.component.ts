import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest, Observable, of } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { take, map } from 'rxjs/operators';
import { AssignmentType, UserAssignment, AssignmentTask, StudentAssignedTask, StudentAssignedTaskState, TaskSubmissionFileType, TaskSubmissionFileTypeText, TaskSubmissionFile, TaskSubmission, TaskSubmissionState } from '../../models/assignment.model';
import { MAT_DATE_LOCALE, MatDialog } from '@angular/material';

import { saveAs } from 'file-saver';
import { AuthService } from '../../authentication/services/auth.service';
import { GradeSubmissionDialog, GradeDialogData, GradeDialogResult } from './grade-assignment.dialog';

@Component({
  selector: 'app-view-assignment-page',
  templateUrl: './view-assignment-page.component.html',
  styleUrls: ['./view-assignment-page.component.scss'],
  animations: []
})
export class ViewAssignmentPageComponent implements OnInit, OnDestroy {

  AssignmentType = AssignmentType;
  StudentAssignedTaskState = StudentAssignedTaskState;
  TaskSubmissionState = TaskSubmissionState;

  routeSub: Subscription;
  assignment: UserAssignment;
  assignedTask: StudentAssignedTask;

  datesShown: boolean = false;

  constructor(private route: ActivatedRoute,
    private modelSvcFactory: ModelServiceFactory,
    private auth: AuthService,
    public dialog: MatDialog,
    @Inject(MAT_DATE_LOCALE) private matDateLocale: string) { }

  courseSlug: string;
  assignmentSlug: string;

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
        this.assignment = assignmentResult;

        if (this.userHasChosenTask) {
          this.getAssignedTask();
        }
      });
  }

  private getAssignedTask() {
    this.modelSvcFactory.studentAssignedTasks.getAssignedTask(this.assignment.id)
      .pipe(take(1))
      .subscribe(studentTask => {
        this.assignedTask = studentTask;
      });
  }

  chooseTask(task: AssignmentTask) {
    this.modelSvcFactory.studentAssignedTasks.assignTaskToSelf(task.id)
      .then(_ => {
        this.getAssignment();
      });
  }

  get submissionsStarted(): boolean {
    return this.compareToNow(this.assignment.startDate) < 0;
  }

  get submissionsEnded(): boolean {
    return this.compareToNow(this.assignment.endDate) < 0;
  }

  get hasTasksList(): boolean {
    return this.assignment.type != AssignmentType.SingleTask
      && this.assignment.type != AssignmentType.CustomAssignedTasks;
  }

  get showTasksList(): boolean {
    return this.hasTasksList && this.assignment.assignedTask == null;
  }

  get userHasChosenTask(): boolean {
    return this.assignment.assignedTask != null;
  }

  taskCanBeChosen(task: AssignmentTask): boolean {
    if (this.userHasChosenTask || this.submissionsEnded)
      return false;

    if (this.assignment.type == AssignmentType.SingleTask || this.assignment.type == AssignmentType.CustomAssignedTasks)
      return false;

    if (this.assignment.type == AssignmentType.SingleChoiceList && task.studentsAssigned.length > 0)
      return false;

    if (this.assignment.type == AssignmentType.MultipleChoiceList && this.assignment.numberOfDuplicates <= task.studentsAssigned.length)
      return false;

    if (this.compareToNow(this.assignment.startDate) > 0)
      return false;

    if (this.compareToNow(this.assignment.endDate) < 0)
      return false;

    return true;
  }

  getDateString(date: Date): string {
    return date.toLocaleString(this.matDateLocale);
  }

  compareDates(date1: Date | number, date2: Date | number): number {
    return date1.valueOf() - date2.valueOf();
  }

  compareToNow(date: Date): number {
    return this.compareDates(date, Date.now());
  }

  getFileTypeString(type: TaskSubmissionFileType): string {
    return TaskSubmissionFileTypeText[TaskSubmissionFileType[type]];
  }

  downloadFile(file: TaskSubmissionFile) {
    // event.preventDefault();

    this.modelSvcFactory.files.download(file.fileId)
      .then(downloadedFile => saveAs(downloadedFile.blob, downloadedFile.fileName || `${file.name}.${file.extension}`))
  }

  get canAddSubmission(): Observable<boolean> {
    if (this.submissionsEnded || this.assignedTask.state == StudentAssignedTaskState.FinalGraded)
      return of(false);
    return this.auth.user$
      .pipe(map(user => this.assignedTask.studentId == user.id))
  }

  get canGrade(): Observable<boolean> {
    if (this.assignedTask.state == StudentAssignedTaskState.FinalGraded)
      return of(false);

    return this.auth.canGradeAssignment();
  }

  get canAddFinalGrade(): Observable<boolean> {
    return this.auth.canGradeAssignment()
      .pipe(map(allowed => {
        if (!allowed)
          return false;

        if (this.assignedTask.state == StudentAssignedTaskState.FinalGraded)
          return false;

        return !!this.lastGradedSubmission;
      }));
  }

  get lastGradedSubmission(): TaskSubmission {

    const submissions = this.assignedTask.submissions;
    for (let i = submissions.length - 1; i > -1; i--) {
      if (submissions[i].state == TaskSubmissionState.Graded)
        return submissions[i];
    }
    return null;
  }

  openGradeDialog(submission: TaskSubmission): void {
    const dialogRef = this.dialog.open<GradeSubmissionDialog, GradeDialogData, GradeDialogResult>(GradeSubmissionDialog, {
      width: '300px',
      data: {
        title: 'Grade submission',
        reviewRequired: true,
        review: submission.review,
        grade: submission.grading
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result)
        return;

      this.modelSvcFactory.submissions.grade(submission.id, {
        review: result.review,
        grade: result.grade
      })
        .then(_ => this.getAssignedTask());
    });
  }

  openFinalGradeDialog(): void {
    const dialogRef = this.dialog.open<GradeSubmissionDialog, GradeDialogData, GradeDialogResult>(GradeSubmissionDialog, {
      width: '300px',
      data: {
        title: 'Confirm final grading',
        grade: this.lastGradedSubmission.grading
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result)
        return;

      this.modelSvcFactory.studentAssignedTasks.confirmFinalGrading(this.assignedTask.id, {
        review: result.review,
        grade: result.grade
      })
        .then(_ => this.getAssignedTask());
    });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
