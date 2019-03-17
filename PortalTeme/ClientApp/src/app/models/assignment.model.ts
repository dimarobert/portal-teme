import { EditModel, BaseModel, NamedModel } from './base.model';
import { CourseRef, User, UserRef } from './course.model';
import { UploadedFile } from '../components/dropzone-file-upload/upload-file.model';

export interface UserAssignment extends Assignment {
    assignedTask: AssignmentTask;
}

export interface Assignment extends AssignmentEdit {
    id: string;

    slug: string;

    tasks: AssignmentTask[];
}

export interface AssignmentEdit extends EditModel {
    course: CourseRef;

    name: string;
    description: string;

    type: AssignmentType;

    numberOfDuplicates: number;

    startDate: Date;
    endDate: Date;

    dateAdded?: Date;
    lastUpdated?: Date;
}

export enum AssignmentType {
    SingleTask,
    SingleChoiceList,
    MultipleChoiceList,
    CustomAssignedTasks
}

export const AssignmentTypeText = {
    SingleTask: "Single task",
    SingleTaskText: "A single exercise/task will be defined for the entire class to solve.",

    SingleChoiceList: "List of tasks - single choice",
    SingleChoiceListText: "A list of exercises/tasks will be defined, each student will have to chose his own task. A task can only be chosen by a single student.",

    MultipleChoiceList: "List of tasks - multiple choice",
    MultipleChoiceListText: "A list of exercises/tasks will be defined, each student will have to chose his own task. A task can be chosen by the specified number of students.",

    CustomAssignedTasks: "Individually assigned tasks",
    CustomAssignedTasksText: "Each task will be manually assigned to each student."
}

export interface AssignmentTaskBase {
    assignmentId: string;

    name: string;
    description: string;
}

export interface AssignmentTaskCreateRequest extends AssignmentTaskBase {
    assignedTo?: string;
}

export function isTaskUpdateRequest(request: any): request is AssignmentTaskUpdateRequest {
    if (request.id)
        return true;
    return false;
}

export interface AssignmentTaskUpdateRequest extends AssignmentTaskCreateRequest {
    id: string;
}

export interface AssignmentTask extends AssignmentTaskEdit {
    id: string;

    studentsAssigned: User[];
}

export interface AssignmentTaskEdit extends AssignmentTaskBase, EditModel {
    studentsAssigned?: UserRef[];
}


export interface StudentAssignedTask extends BaseModel {

    task: AssignmentTask;

    studentId: string;
    student: User;

    state: StudentAssignedTaskState;
    review: string;
    finalGrading?: number;

    submissions: TaskSubmission[];
}

export enum StudentAssignedTaskState {
    Assigned,
    FinalGraded
}

export interface CreateTaskSubmissionRequest {
    studentTaskId: string;
    uploadedFiles: UploadedFile[];
    description: string;
}

export interface GradeTaskSubmissionRequest {
    review: string;
    grade: number;
}

export interface TaskSubmission extends EditModel {
    studentTaskId: string;
    dateAdded: Date;
    state: TaskSubmissionState;
    description: string;
    review: string;
    grading?: number;
    files: TaskSubmissionFile[];
}

export enum TaskSubmissionState {
    Submitted,
    Reviewed,
    Graded
}

export interface TaskSubmissionFile extends NamedModel {
    fileId: string;
    extension: string;
    size: number;
    description: string;
    fileType: TaskSubmissionFileType;
}

export enum TaskSubmissionFileType {
    SourceCode,
    Project,
    Essay
}

export const TaskSubmissionFileTypeText = {
    SourceCode: "Source code",
    Project: "Project",
    Essay: "Essay"
}