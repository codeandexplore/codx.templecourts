import { test, expect } from "@playwright/test";

test.describe("Auth", () => {
  test("login page renders", async ({ page }) => {
    await page.goto("/login");
    await expect(page.getByText("Sign in").first()).toBeVisible();
    await expect(page.locator('input[name="email"]')).toBeVisible();
    await expect(page.locator('input[name="password"]')).toBeVisible();
  });

  test("register page renders", async ({ page }) => {
    await page.goto("/register");
    await expect(page.getByText("Create account").first()).toBeVisible();
    await expect(page.locator('input[name="displayName"]')).toBeVisible();
  });

  test("redirects to login when unauthenticated", async ({ page }) => {
    await page.goto("/lessons");
    await page.waitForURL("/login");
  });

  test("can login with valid credentials", async ({ page }) => {
    await page.goto("/login");
    await page.locator('input[name="email"]').fill("admin@templecourts.local");
    await page.locator('input[name="password"]').fill("Admin123!");
    await page.getByRole("button", { name: "Sign in" }).click();
    await page.waitForURL("/");
    await expect(page.getByText("Welcome").first()).toBeVisible();
  });
});
