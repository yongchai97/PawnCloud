import { NgModule } from '@angular/core';
import { GoldTypesComponent } from './gold-types.component';
import { GoldTypesRoutingModule } from './gold-types-routing.module';

@NgModule({
    imports: [GoldTypesRoutingModule, GoldTypesComponent],
})
export class GoldTypesModule {}
