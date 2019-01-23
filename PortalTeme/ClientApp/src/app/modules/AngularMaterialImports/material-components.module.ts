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
  MatExpansionModule
} from '@angular/material';

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
})
export class MaterialComponentsModule { }
