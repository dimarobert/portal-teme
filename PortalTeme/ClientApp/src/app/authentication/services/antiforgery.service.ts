import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AntiforgeryService {

  constructor() { }

  public getAntiforgeryToken(): string {
    return document.querySelector<HTMLInputElement>('input[name="__RequestVerificationToken"]').value;
  }

  public getHeaderName(): string {
    return 'RequestVerificationToken';
  }
}
