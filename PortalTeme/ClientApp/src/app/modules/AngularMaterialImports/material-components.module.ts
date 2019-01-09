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
  MatTabsModule
} from '@angular/material';

const importedMaterialComponents = [
  CdkAccordionModule,

  MatTooltipModule,
  MatButtonModule,
  MatIconModule,

  MatMenuModule,
  MatListModule,

  MatToolbarModule,
  MatSidenavModule,
  MatTabsModule,

  MatCardModule,

  MatInputModule,
  MatSelectModule,
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
