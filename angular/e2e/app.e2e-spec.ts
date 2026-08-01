import { PawnCloudTemplatePage } from './app.po';

describe('PawnCloud App', function () {
    let page: PawnCloudTemplatePage;

    beforeEach(() => {
        page = new PawnCloudTemplatePage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('app works!');
    });
});
