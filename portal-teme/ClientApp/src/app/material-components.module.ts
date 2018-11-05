import { NgModule } from '@angular/core';

import { MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, MatListModule, MatTableModule, MatSortModule } from '@angular/material';

const importedMaterialComponents = [
  MatToolbarModule,
  MatButtonModule,
  MatIconModule,
  MatSidenavModule,
  MatListModule,
  MatSortModule,
  MatTableModule
];

@NgModule({
  imports: importedMaterialComponents,
  exports: importedMaterialComponents,
})
export class MaterialComponentsModule { }
