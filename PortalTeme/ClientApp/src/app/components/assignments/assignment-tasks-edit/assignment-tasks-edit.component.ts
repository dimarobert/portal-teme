import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { AssignmentTask, AssignmentTaskEdit } from '../../../models/assignment.model';

@Component({
  selector: 'app-assignment-tasks-edit',
  templateUrl: './assignment-tasks-edit.component.html',
  styleUrls: ['./assignment-tasks-edit.component.scss']
})
export class AssignmentTasksEditComponent implements OnInit, OnDestroy {

  @Input() tasks: Observable<AssignmentTask[]>;
  @Input() assignmentId: string;

  @Input() create: (newTask: AssignmentTaskEdit) => Promise<AssignmentTask>;
  @Input() update: (task: AssignmentTask) => Promise<void>;
  @Input() delete: (task: AssignmentTask) => Promise<void>;

  tasksList: BehaviorSubject<AssignmentTaskEdit[]>;
  tasksSub: Subscription;

  constructor() { }

  ngOnInit() {

    this.tasksList = new BehaviorSubject([]);

    this.tasksSub = this.tasks.subscribe(t => this.tasksList.next(t));
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

  submitClick(): (task: AssignmentTaskEdit) => Promise<void> {
    return (task) => {
      if (task.id)
        return this.update(<AssignmentTask>task);
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
