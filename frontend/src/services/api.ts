import { apiBaseUrl } from "../config/api";
import type { LoginResult, Pet, SavePet } from "../types/pet";
const request = async <T>(
  path: string,
  options: RequestInit = {},
  token?: string,
): Promise<T> => {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });
  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(
      body?.message ??
        (response.status === 401
          ? "Your session has expired."
          : "Request failed."),
    );
  }
  return response.status === 204
    ? (undefined as T)
    : (response.json() as Promise<T>);
};
export const login = (username: string, password: string) =>
  request<LoginResult>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ username, password }),
  });
export const getPets = (token: string) => request<Pet[]>("/pets", {}, token);
export const createPet = (pet: SavePet, token: string) =>
  request<Pet>("/pets", { method: "POST", body: JSON.stringify(pet) }, token);
export const updatePet = (id: string, pet: SavePet, token: string) =>
  request<Pet>(
    `/pets/${id}`,
    { method: "PUT", body: JSON.stringify(pet) },
    token,
  );
export const deletePet = (id: string, token: string) =>
  request<void>(`/pets/${id}`, { method: "DELETE" }, token);
