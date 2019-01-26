import { NgModule } from '@angular/core';

import {
  CdkAccordionModule
} from '@angular/cdk/accordion';

import {
  MatToolbarModule,
  MatButtonModule,
  MatIconModule,
  MatSidenavModule,
  MatListModule,
  MatTableModule,
  MatSortModule,
  MatMenuModule,
  MatTooltipModule,
  MatFormFieldModule,
  MatInputModule,
  MatCardModule,

  MatSlideToggleModule,
  MatSelectModule,
  MatTabsModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatExpansionModule,
  DateAdapter,
  MAT_DATE_LOCALE
} from '@angular/material';
import { CustomDateAdapter, DateAdapterService } from './date.adapter';

const importedMaterialComponents = [
  CdkAccordionModule,

  MatTooltipModule,
  MatButtonModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,

  MatMenuModule,
  MatListModule,

  MatToolbarModule,
  MatSidenavModule,
  MatTabsModule,

  MatCardModule,
  MatExpansionModule,

  MatInputModule,
  MatSelectModule,

  MatDatepickerModule,
  MatNativeDateModule,

  MatFormFieldModule,
  MatSlideToggleModule,

  MatSortModule,
  MatTableModule


];

@NgModule({
  imports: importedMaterialComponents,
  exports: importedMaterialComponents,
  providers: [
    {provide: MAT_DATE_LOCALE, useValue: 'ro-RO'},
    { provide: DateAdapter, useClass: CustomDateAdapter }
  ]
})
export class MaterialComponentsModule { }
