import { GoBuddiesAngularPage } from './app.po';

describe('go-buddies-angular App', function() {
  let page: GoBuddiesAngularPage;

  beforeEach(() => {
    page = new GoBuddiesAngularPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
