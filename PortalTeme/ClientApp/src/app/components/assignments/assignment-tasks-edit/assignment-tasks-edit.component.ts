import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { AssignmentTask, AssignmentTaskEdit, AssignmentTaskCreateRequest, isTaskUpdateRequest, AssignmentTaskUpdateRequest } from '../../../models/assignment.model';
import { User } from '../../../models/course.model';

@Component({
  selector: 'app-assignment-tasks-edit',
  templateUrl: './assignment-tasks-edit.component.html',
  styleUrls: ['./assignment-tasks-edit.component.scss']
})
export class AssignmentTasksEditComponent implements OnInit, OnDestroy {

  @Input() courseStudents: User[];
  @Input() assignmentId: string;
  @Input() tasks: Observable<AssignmentTask[]>;
  @Input() isCustomAssigned: boolean;

  @Input() create: (newTask: AssignmentTaskCreateRequest) => Promise<AssignmentTask>;
  @Input() update: (task: AssignmentTaskUpdateRequest) => Promise<void>;
  @Input() delete: (task: AssignmentTask) => Promise<void>;

  tasksList: BehaviorSubject<AssignmentTaskEdit[]>;
  tasksSub: Subscription;

  constructor() { }

  ngOnInit() {

    this.tasksList = new BehaviorSubject([]);

    this.tasksSub = this.tasks.subscribe(t => this.tasksList.next(t));
  }

  showAssignedStudent(task: AssignmentTaskEdit): boolean {
    if (task.studentsAssigned.length == 1){
      return true;
    }
  }

  add() {
    let newTasks = this.tasksList.value.slice();
    newTasks.push({
      name: 'New Task',
      assignmentId: this.assignmentId,
      description: ''
    });
    this.tasksList.next(newTasks);
  }

  submitClick(): (task: AssignmentTaskCreateRequest) => Promise<void> {
    return (task) => {
      if (isTaskUpdateRequest(task))
        return this.update(task);
      else
        return this.create(task)
          .then(createdTask => {
            let updatedTasks = this.tasksList.value.slice();
            const idx = updatedTasks.findIndex(t => t == task);
            updatedTasks[idx] = createdTask;
            this.tasksList.next(updatedTasks);
          });
    }
  }

  deleteTask(): (task: AssignmentTaskEdit) => void {
    return (task) => {
      if (task.id)
        this.delete(<AssignmentTask>task);

      let updatedTasks = this.tasksList.value.slice();
      const idx = updatedTasks.findIndex(t => t == task);
      updatedTasks.splice(idx, 1);
      this.tasksList.next(updatedTasks);
    };
  }

  ngOnDestroy(): void {
    this.tasksSub && this.tasksSub.unsubscribe();
  }
}
