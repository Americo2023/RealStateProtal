export const apiUrl = import.meta.env.VITE_API_URL ?? "http://localhost:5080";

export const request = async <T>(path: string, options?: RequestInit): Promise<T> => {
  const response = await fetch(`${apiUrl}${path}`, options);
  if (!response.ok) {
    throw new Error(`La API respondió con ${response.status}.`);
  }
  return (await response.json()) as T;
};
