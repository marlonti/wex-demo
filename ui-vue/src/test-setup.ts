// Vuetify's overlay/positioning components (VDialog, VMenu, VOverlay, ...) touch a few
// browser APIs jsdom doesn't implement. Stub them so components mount in tests.
class ResizeObserverStub {
  observe() {}
  unobserve() {}
  disconnect() {}
}

window.ResizeObserver ??= ResizeObserverStub as unknown as typeof ResizeObserver

if (!window.visualViewport) {
  Object.defineProperty(window, 'visualViewport', {
    writable: true,
    value: {
      addEventListener: () => {},
      removeEventListener: () => {},
      width: window.innerWidth,
      height: window.innerHeight,
    },
  })
}

window.matchMedia ??= (query: string) =>
  ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => {},
    removeListener: () => {},
    addEventListener: () => {},
    removeEventListener: () => {},
    dispatchEvent: () => false,
  }) as unknown as MediaQueryList
