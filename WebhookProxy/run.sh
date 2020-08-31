#!/bin/bash
smee_url="`cat /WebhookProxy/smeeurl`"
smee -u $smee_url -t http://host.docker.internal:3000/automation/webhook