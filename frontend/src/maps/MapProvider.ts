export type MapCoordinates = {
  latitude: number
  longitude: number
}

export interface MapProvider {
  render(container: HTMLElement, coordinates: MapCoordinates): void
}
