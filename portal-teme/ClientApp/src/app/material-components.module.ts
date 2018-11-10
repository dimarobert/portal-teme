import { NgModule } from '@angular/core';

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
  MatSlideToggleModule
} from '@angular/material';

const importedMaterialComponents = [
  MatTooltipModule,
  MatButtonModule,
  MatIconModule,

  MatMenuModule,
  MatListModule,

  MatToolbarModule,
  MatSidenavModule,

  MatCardModule,

  MatInputModule,
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
