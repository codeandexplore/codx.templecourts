import { test as base, expect } from "@playwright/test";

const API_URL = process.env.API_URL || "http://localhost:5000";

export interface AuthFixtures {
  adminAuth: { token: string; user: object };
  studentAuth: { token: string; user: object };
}

async function getToken(email: string, password: string) {
  const res = await fetch(`${API_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  if (!res.ok) {
    const registerRes = await fetch(`${API_URL}/auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password, displayName: email.split("@")[0] }),
    });
    if (!registerRes.ok) throw new Error(`Failed to register: ${registerRes.status}`);
    const data = await registerRes.json();
    return data as { accessToken: string; user: object };
  }
  return await res.json() as { accessToken: string; user: object };
}

export const test = base.extend<AuthFixtures>({
  adminAuth: async ({ page }, use) => {
    const adminEmail = process.env.E2E_ADMIN_EMAIL || "admin@templecourts.test";
    const adminPassword = process.env.E2E_ADMIN_PASSWORD || "Admin123!";
    const auth = await getToken(adminEmail, adminPassword);
    await page.addInitScript((token) => {
      localStorage.setItem("auth_token", token);
    }, auth.accessToken);
    await use(auth);
  },
});

export { expect };
