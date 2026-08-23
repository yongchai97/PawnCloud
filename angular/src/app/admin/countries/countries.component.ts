import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { Country, CountryServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './countries.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, NgIf, TableModule, PaginatorModule, ButtonModule, LocalizePipe],
})
export class CountriesComponent extends AppComponentBase implements OnInit {
    year = new Date().getFullYear();
    countries: Country[] = [];
    displayedCountries: Country[] = [];
    loading = false;
    rows = 10;
    first = 0;

    constructor(
        injector: Injector,
        private countryService: CountryServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadCountries();
    }

    loadCountries(): void {
        this.loading = true;
        this.first = 0;
        this.countryService
            .getAllViaYear(this.year)
            .pipe(finalize(() => (this.loading = false)))
            .subscribe((result) => {
                this.countries = result || [];
                this.updateDisplayedCountries();
                this.cd.detectChanges();
            });
    }

    onPageChange(event: { first?: number; rows?: number }): void {
        this.first = event.first || 0;
        this.rows = event.rows || this.rows;
        this.updateDisplayedCountries();
    }

    private updateDisplayedCountries(): void {
        this.displayedCountries = this.countries.slice(this.first, this.first + this.rows);
    }
}
