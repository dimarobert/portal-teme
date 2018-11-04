import { NgModule } from '@angular/core';

import { MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, MatListModule } from '@angular/material';

const importedMaterialComponents = [
  MatToolbarModule,
  MatButtonModule,
  MatIconModule,
  MatSidenavModule,
  MatListModule
];

@NgModule({
  imports: importedMaterialComponents,
  exports: importedMaterialComponents,
})
export class MaterialComponentsModule { }
