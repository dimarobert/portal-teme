import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Year } from '../models/year.model';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CoursesService {

  constructor(private http: HttpClient) { }

  getYears(): Observable<Year[]> {
    return this.http.get<Year[]>('/api/AcademicYears')
  }

  saveYear(year: Year): Promise<Year> {
    return this.http.post<Year>('api/AcademicYears', year)
      .pipe(take(1))
      .toPromise();
  }

  deleteYear(year: Year): Promise<Year> {
    return this.http.delete<Year>(`api/AcademicYears/${year.id}`)
      .pipe(take(1))
      .toPromise();
  }
}
