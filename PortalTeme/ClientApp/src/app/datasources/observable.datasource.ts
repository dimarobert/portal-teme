import { MatTableDataSource } from '@angular/material';
import { Subscription, Observable, BehaviorSubject } from 'rxjs';

export class ObservableDataSource<T> extends MatTableDataSource<T> {
    dataSubscription: Subscription;
  
    constructor(private dataObservable: Observable<T[]>) {
      super();
    }
  
    connect(): BehaviorSubject<T[]> {
      this.dataSubscription = this.dataObservable.subscribe({
        next: (v) => {
          this.data = v;
        }
      });
  
      return super.connect();
    }
  
    disconnect(): void {
      this.dataSubscription.unsubscribe();
    }
  
  }