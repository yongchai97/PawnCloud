import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SeederComponent } from './seeder.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: SeederComponent,
                data: { title: 'Seeder' },
            },
        ]),
    ],
})
export class SeederRoutingModule {}
