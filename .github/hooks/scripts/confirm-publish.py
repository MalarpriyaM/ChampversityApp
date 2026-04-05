"""
PreToolUse hook — pauses and requests human confirmation before any Jira or Confluence write operation.
Prevents accidental publishing of draft or unreviewed BA/PO artifacts.
"""
import sys
import json

data = json.load(sys.stdin)
tool = data.get("tool_name", "")

# All Atlassian tools that create or mutate content
WRITE_TOOLS = {
    "mcp_com_atlassian_createJiraIssue",
    "mcp_com_atlassian_editJiraIssue",
    "mcp_com_atlassian_transitionJiraIssue",
    "mcp_com_atlassian_addCommentToJiraIssue",
    "mcp_com_atlassian_addWorklogToJiraIssue",
    "mcp_com_atlassian_createIssueLink",
    "mcp_com_atlassian_createConfluencePage",
    "mcp_com_atlassian_updateConfluencePage",
    "mcp_com_atlassian_createConfluenceFooterComment",
    "mcp_com_atlassian_createConfluenceInlineComment",
    "mcp_com_atlassian_createCompassComponent",
    "mcp_com_atlassian_createCompassComponentRelationship",
}

if tool in WRITE_TOOLS:
    result = {
        "hookSpecificOutput": {
            "hookEventName": "PreToolUse",
            "permissionDecision": "ask",
            "permissionDecisionReason": (
                f"'{tool}' will create or modify content in Jira or Confluence. "
                "Review the generated output above and confirm it is ready to publish."
            )
        }
    }
else:
    result = {
        "hookSpecificOutput": {
            "hookEventName": "PreToolUse",
            "permissionDecision": "allow"
        }
    }

print(json.dumps(result))
