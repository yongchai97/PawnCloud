import { Injector, Pipe, PipeTransform } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Pipe({
    name: 'localize',
    standalone: true,
})
export class LocalizePipe extends AppComponentBase implements PipeTransform {
    constructor(injector: Injector) {
        super(injector);
    }

    transform(key: string, ...args: any[]): string {
        const localizedText = this.l(key, ...args);
        return localizedText === key ? key.replace(/([a-z])([A-Z])/g, '$1 $2').replace(/([A-Z]+)([A-Z][a-z])/g, '$1 $2') : localizedText;
    }
}
