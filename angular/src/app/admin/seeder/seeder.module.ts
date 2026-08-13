import { NgModule } from '@angular/core';
import { SeederComponent } from './seeder.component';
import { SeederRoutingModule } from './seeder-routing.module';

@NgModule({
    imports: [SeederRoutingModule, SeederComponent],
})
export class SeederModule {}
