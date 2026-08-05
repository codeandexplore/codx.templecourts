import { test, expect } from "@playwright/test";

test.describe("Auth", () => {
  test("login page renders", async ({ page }) => {
    await page.goto("/login");
    await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
    await expect(page.getByLabel("Email")).toBeVisible();
    await expect(page.getByLabel("Password")).toBeVisible();
  });

  test("register page renders", async ({ page }) => {
    await page.goto("/register");
    await expect(page.getByRole("heading", { name: "Create account" })).toBeVisible();
    await expect(page.getByLabel("Display Name")).toBeVisible();
  });

  test("redirects to login when unauthenticated", async ({ page }) => {
    await page.goto("/lessons");
    await page.waitForURL("/login");
  });

  test("can login with valid credentials", async ({ page }) => {
    await page.goto("/login");
    await page.fill('input[name="email"]', "admin@templecourts.test");
    await page.fill('input[name="password"]', "Admin123!");
    await page.click('button[type="submit"]');
    await page.waitForURL("/");
    await expect(page.getByRole("heading", { name: "Welcome" })).toBeVisible();
  });
});
