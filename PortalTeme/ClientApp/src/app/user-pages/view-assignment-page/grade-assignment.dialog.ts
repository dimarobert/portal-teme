import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { nameof } from '../../type-guards/nameof.guard';

export interface GradeDialogData {
    title: string;
    review?: string;
    reviewRequired?: boolean;
    grade: number;
}

export interface GradeDialogResult {
    review?: string;
    grade: number;
}

@Component({
    selector: 'grade-assignment-dialog',
    templateUrl: 'grade-assignment.dialog.html',
})
export class GradeSubmissionDialog implements OnInit {

    constructor(
        public dialogRef: MatDialogRef<GradeSubmissionDialog>,
        @Inject(MAT_DIALOG_DATA) public data: GradeDialogData) {
    }

    form: FormGroup;


    ngOnInit(): void {
        this.form = new FormGroup({});

        this.form.addControl(nameof<GradeDialogData>('review'), new FormControl(this.data.review))
        this.form.addControl(nameof<GradeDialogData>('grade'), new FormControl(this.data.grade || 1))
    }

    get review(): AbstractControl {
        return this.form.get(nameof<GradeDialogData>('review'));
    }


    get grade(): AbstractControl {
        return this.form.get(nameof<GradeDialogData>('grade'));
    }

    onNoClick(): void {
        this.dialogRef.close();
    }

    submitDialog() {
        if (this.form.invalid)
            return;

        this.dialogRef.close({
            review: this.review.value,
            grade: this.grade.value
        });
    }

}
