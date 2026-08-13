import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GoldTypesComponent } from './gold-types.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: GoldTypesComponent,
                data: { title: 'GoldTypes' },
            },
        ]),
    ],
})
export class GoldTypesRoutingModule {}
