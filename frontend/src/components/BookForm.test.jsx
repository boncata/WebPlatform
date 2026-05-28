// vi is Vitest utility namespace, similar to mocking helpers.
import { describe, test, expect, vi, afterEach } from "vitest";

import { render, screen, fireEvent, waitFor, cleanup } from "@testing-library/react";

import BookForm from "./BookForm";

// Cleans up the DOM & Mocks after each test.
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});


// Mock the createBook function, because it has a real axios api call, i.e.
// when BookForm imports this module, replace it with fake version.
import { createBook } from "../api/books";

vi.mock("../api/books", () => {
  return {
    createBook: vi.fn(() => Promise.resolve())
  };
});

describe("BookForm", () => {

  /**
   * Test that form submits successfully with correct book data and
   * calls the callback.
   */
  test("calls callback after successful submit + correct book data", async () => {
    // Create mock callback function. The mock also gathers test info.
    const mockOnBookCreated = vi.fn();

    // Renders Test (fake) DOM and containing BookForm and passes
    // mocked function to it.
    render(
      <BookForm onBookCreated={mockOnBookCreated} />
    );

    // Hint: Real browser input events look like: event.target.value.
    // React receives same structure. 

    // Simulate user typing. 
    fireEvent.change(
      screen.getByPlaceholderText("Title"), // finds <input placeholder="Title" />
      // This is the event object, which simulates browser event.
      // Equivalent to the user typing "Clean Code".
      {
        target: { value: "Clean Code" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Author"),
      {
        target: { value: "Robert C. Martin" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Price"),
      {
        target: { value: "30" }
      }
    );

    // Simulates click of a button.
    fireEvent.click(
      screen.getByText("Add Book")
    );

    // Verify that callback function was executed.
    // "waitFor" is needed, because form submission is asynchronous.
    // It will keep checking until assertion passes or timeout occurs.
    // If we use "expect" the assertion might run before async code finishes. 
    await waitFor(() => {
      expect(createBook).toHaveBeenCalledWith({
        title: "Clean Code",
        author: "Robert C. Martin",
        price: 30
      });

      // Here we can use "expect" because mockOnBookCreated is called synchronously
      // after createBook.query is resolved.
      expect(mockOnBookCreated).toHaveBeenCalled();
    });
  });

  /**
   * Test that form does not submit when fields are empty. This is important,
   * because otherwise we would have many invalid entries in the database.
   * We also check that alert is shown with correct message, because user should
   * get feedback about what went wrong.
   */
  test("does not submit when fields are empty", async () => {
    const mockOnBookCreated = vi.fn();

    // Spy on (browser)window.alert to check if it's called with correct message.
    // Replace alert with empty function, via mockImplementation, so 
    // it doesn't actually pop up during tests.
    const alertSpy = vi
      .spyOn(window, "alert")
      .mockImplementation(() => {});
      
    render(
      <BookForm onBookCreated={mockOnBookCreated} />
    );

    // Simulate click of a button without filling the form.
    fireEvent.click(
      screen.getByText("Add Book")
    );

    // Verify that createBook is not called, because form is invalid.
    await waitFor(() => {
      expect(createBook).not.toHaveBeenCalled();

      // Verify that callback is not called, because book creation should fail.
      expect(mockOnBookCreated)
        .not.toHaveBeenCalled();

        // Verify that alert is called with correct message.
      expect(alertSpy)
        .toHaveBeenCalledWith(
          "All fields are required."
        );
    });

    // Restores original browser alert function. Prevents test pollution.
    alertSpy.mockRestore();
  });
});