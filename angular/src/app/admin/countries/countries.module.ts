import { NgModule } from '@angular/core';
import { CountriesComponent } from './countries.component';
import { CountriesRoutingModule } from './countries-routing.module';

@NgModule({
    imports: [CountriesRoutingModule, CountriesComponent],
})
export class CountriesModule {}
