import { Component, OnInit } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';

@Component({
  selector: 'app-error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.scss']
})
export class ErrorPageComponent implements OnInit {

  constructor(private route: ActivatedRouteSnapshot) { }

  get message(): string | null {
    return this.route.queryParamMap.get("message");
  }

  ngOnInit() {
  }

}
