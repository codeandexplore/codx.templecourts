import { test, expect } from "@playwright/test";

test.describe("Lessons", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/login");
    await page.locator('input[name="email"]').fill("admin@templecourts.local");
    await page.locator('input[name="password"]').fill("Admin123!");
    await page.getByRole("button", { name: "Sign in" }).click();
    await page.waitForURL("/");
  });

  test("lesson list shows published lessons", async ({ page }) => {
    await page.click('a[href="/lessons"]');
    await expect(page.getByText("Lessons")).toBeVisible();
    const lessonLinks = page.locator('a[href^="/lessons/"]');
    await expect(lessonLinks.first()).toBeVisible();
  });

  test("lesson detail renders tree structure", async ({ page }) => {
    await page.click('a[href="/lessons"]');
    const firstLesson = page.locator('a[href^="/lessons/"]').first();
    await firstLesson.click();
    await expect(page.getByText("Lesson Content")).toBeVisible();
  });

  test("can start a lesson attempt", async ({ page }) => {
    await page.click('a[href="/lessons"]');
    const firstLesson = page.locator('a[href^="/lessons/"]').first();
    await firstLesson.click();
    const startBtn = page.getByRole("button", { name: "Start Lesson" });
    if (await startBtn.isVisible()) {
      await startBtn.click();
      await expect(page.getByRole("button", { name: "Continue Lesson" })).toBeVisible();
    }
  });
});
