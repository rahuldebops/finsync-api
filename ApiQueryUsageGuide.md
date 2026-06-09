# API Usage Guide: Paging, Sorting, and Filtering

This guide explains how to retrieve data from any resource endpoint (e.g., `/api/groups`, `/api/users`) using a standardized set of query parameters.

**Base Endpoint:** `GET /api/{resource}`

---

## 1. Pagination

Control the size and position of the data subset you retrieve.

| Parameter  | Type    | Default | Description                               |
| :--------- | :------ | :------ | :---------------------------------------- |
| `page`     | Integer | `1`     | The page number you want to retrieve.     |
| `pageSize` | Integer | `10`    | The number of items to include per page.  |

### Use Cases
*   **Get the first page of default size (10 items):**
    ```
    GET /api/groups
    ```

*   **Get the first page with 25 items:**
    ```
    GET /api/groups?pageSize=25
    ```

*   **Get the third page with 25 items:**
    ```
    GET /api/groups?page=3&pageSize=25
    ```

---

## 2. Sorting

Define the order in which results should be returned. You can sort by multiple fields.

*   **Parameter:** `sort`
*   **Syntax:** `sort={field1},-{field2},{field3}`
    *   A field name by itself (e.g., `name`) implies **ascending** order.
    *   A field name prefixed with a hyphen (`-`) (e.g., `-createdAt`) implies **descending** order.
    *   Multiple fields are separated by a comma (`,`) and are applied in the order they appear.

### Use Cases
*   **Sort by name in ascending order:**
    ```
    GET /api/groups?sort=name
    ```

*   **Sort by creation date in descending order (newest first):**
    ```
    GET /api/groups?sort=-createdAt
    ```

*   **Sort by status descending, then by name ascending:**
    ```
    GET /api/groups?sort=-status,name
    ```

---

## 3. Filtering

Precisely select which data to retrieve by applying conditions to specific fields.

*   **Parameter:** `filter`
*   **Syntax:** `filter[{field}][{operator}]={value}`
    *   `{field}`: The name of the property you want to filter on (e.g., `status`).
    *   `{operator}`: (Optional) The comparison operator to use. If omitted, it defaults to `equals`.
    *   `{value}`: The value to compare against.

### Supported Operators

| Operator         | Description                          | Example                                                        |
| :--------------- | :----------------------------------- | :------------------------------------------------------------- |
| `equals`         | Equals (default if omitted)          | `filter[status]=active`                                        |
| `notequals`      | Not equals                           | `filter[status][notequals]=archived`                           |
| `contains`       | String contains the value (ILIKE)    | `filter[name][contains]=project`                               |
| `notcontains`    | String does not contain (NOT ILIKE)  | `filter[name][notcontains]=test`                               |
| `startswith`     | String starts with (ILIKE)           | `filter[name][startswith]=fin`                                 |
| `endswith`       | String ends with (ILIKE)             | `filter[name][endswith]=sync`                                  |
| `greaterthan`    | Greater than                         | `filter[memberCount][greaterthan]=10`                          |
| `greaterthanorequal` | Greater than or equal to         | `filter[createdAt][greaterthanorequal]=2025-08-01`             |
| `lessthan`       | Less than                            | `filter[priority][lessthan]=5`                                 |
| `lessthanorequal`| Less than or equal to                | `filter[budget][lessthanorequal]=50000`                        |
| `between`        | Between two values (inclusive)       | `filter[amount][between]=100,500`                              |
| `in`             | Matches any value in a list          | `filter[status][in]=active,pending`                            |
| `notin`          | Does not match any value in a list   | `filter[status][notin]=archived,deleted`                       |
| `isnull`         | Value is NULL                        | `filter[description][isnull]=true`                             |
| `isnotnull`      | Value is not NULL                    | `filter[description][isnotnull]=true`                          |
| `isempty`        | Value is NULL or empty string        | `filter[notes][isempty]=true`                                  |
| `isnotempty`     | Value is not NULL and not empty      | `filter[notes][isnotempty]=true`                               |

### Use Cases

*   **Get all groups with the status "active":**
    ```
    GET /api/groups?filter[status]=active
    ```

*   **Get all groups whose name contains the word "Internal":**
    ```
    GET /api/groups?filter[name][contains]=Internal
    ```

*   **Get all groups whose name starts with "Fin":**
    ```
    GET /api/groups?filter[name][startswith]=Fin
    ```

*   **Get all groups created on or after August 1st, 2025:**
    ```
    GET /api/groups?filter[createdAt][greaterthanorequal]=2025-08-01
    ```

*   **Get all transactions with amount between 100 and 500:**
    ```
    GET /api/transactions?filter[amount][between]=100,500
    ```

*   **Get all groups with status in a specific set:**
    ```
    GET /api/groups?filter[status][in]=active,pending
    ```

*   **Get all items where description is not empty:**
    ```
    GET /api/groups?filter[description][isnotempty]=true
    ```

*   **Apply multiple filters (returns items matching ALL conditions):**
    *Get active groups with more than 5 members.*
    ```
    GET /api/groups?filter[status]=active&filter[memberCount][greaterthan]=5
    ```

---

## 4. Combined Queries: The Full Power

You can combine all parameters to build powerful and precise queries.

### Use Case: A Complex Request

*   **Goal:** Find groups whose names contain "API", have an "active" status, and have a priority level less than or equal to 3. Return the **second page** of results, with **5 items per page**, sorted by **priority descending**, then by **name ascending**.

*   **Request:**
    ```
    GET /api/groups?page=2&pageSize=5&sort=-priority,name&filter[name][contains]=API&filter[status]=active&filter[priority][lessthanorequal]=3
    ```

