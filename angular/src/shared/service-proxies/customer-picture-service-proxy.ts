import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL, CustomerPicture } from './service-proxies';

interface AjaxResponse<T> {
    result: T;
    __abp?: boolean;
}

@Injectable()
export class CustomerPictureServiceProxy {
    constructor(
        private http: HttpClient,
        @Optional() @Inject(API_BASE_URL) private baseUrl?: string
    ) {}

    private url(path: string): string {
        return (this.baseUrl ?? '') + '/api/services/app/CustomerPicture' + path;
    }

    upload(customerId: number, file: File): Observable<CustomerPicture> {
        const formData = new FormData();
        formData.append('Customer', customerId.toString());
        formData.append('File', file, file.name);

        return this.http.post<AjaxResponse<CustomerPicture> | CustomerPicture>(this.url('/Upload'), formData).pipe(
            map((response) => {
                const picture = response && '__abp' in response ? response.result : response;
                return CustomerPicture.fromJS(picture);
            })
        );
    }

    getList(customerId: number): Observable<CustomerPicture[]> {
        const params = new HttpParams().set('customer', customerId.toString());
        return this.http.get<AjaxResponse<CustomerPicture[]> | CustomerPicture[]>(this.url('/GetList'), { params }).pipe(
            map((response) => {
                const pictures = Array.isArray(response) ? response : response?.result;
                return (pictures || []).map((picture) => CustomerPicture.fromJS(picture));
            })
        );
    }

    download(id: number): Observable<Blob> {
        const params = new HttpParams().set('id', id.toString());
        return this.http.post(this.url('/Download'), null, { params, responseType: 'blob' });
    }

    delete(id: number): Observable<void> {
        const params = new HttpParams().set('id', id.toString());
        return this.http.delete<void>(this.url('/Delete'), { params });
    }
}
