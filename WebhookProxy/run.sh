#!/bin/bash
smee_url="`cat /WebhookProxy/smeeurl`"
smee -u $smee_url -t http://supervisor:3000/automation/webhook
