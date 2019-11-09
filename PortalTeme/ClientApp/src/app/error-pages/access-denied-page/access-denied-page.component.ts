import { Component, OnInit } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';

@Component({
  selector: 'app-access-denied-page',
  templateUrl: './access-denied-page.component.html',
  styleUrls: ['./access-denied-page.component.scss']
})
export class AccessDeniedPageComponent implements OnInit {

  constructor(private route: ActivatedRouteSnapshot) { }

  get scheme(): string | null {
    return this.route.queryParamMap.get('scheme');
  }

  ngOnInit() {
  }

}
