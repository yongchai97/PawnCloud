import { Component, Injector } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ButtonModule } from 'primeng/button';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CustomSeederServiceProxy } from '@shared/service-proxies/service-proxies';
import { switchMap } from 'rxjs/operators';

@Component({
    selector: 'app-seeder',
    templateUrl: './seeder.component.html',
    standalone: true,
    imports: [CommonModule, ButtonModule, LocalizePipe],
    animations: [appModuleAnimation()],
})
export class SeederComponent extends AppComponentBase {
    isSeedingRootDb = false;

    constructor(
        injector: Injector,
        private seederService: CustomSeederServiceProxy
    ) {
        super(injector);
    }

    seedRootDb(): void {
        if (confirm(this.l('ConfirmSeedRootDb'))) {
            this.isSeedingRootDb = true;

            this.seederService.seedRootDb().pipe(switchMap(() => this.seederService.seedSubDb())).subscribe(
                () => {
                    this.isSeedingRootDb = false;
                    abp.notify.success(this.l('SeedingCompletedSuccessfully'));
                },
                (error) => {
                    this.isSeedingRootDb = false;
                    const message = error.error?.message || this.l('SeedingFailed');
                    abp.notify.error(message);
                }
            );
        }
    }
}
